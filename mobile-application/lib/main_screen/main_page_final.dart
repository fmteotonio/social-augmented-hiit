import 'dart:async';

import 'package:ble/main_screen/components/bottom_button.dart';
import 'package:ble/main_screen/components/heart_rate_display.dart';
import 'package:ble/main_screen/components/input_numpad.dart';
import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_blue/flutter_blue.dart';
import 'package:ble/components/function/device_entry.dart';
import 'package:modal_progress_hud/modal_progress_hud.dart';
import '../constants.dart';
import 'components/scan_button.dart';
import 'package:wakelock/wakelock.dart';
import 'dart:io';

enum AppMode { inputNumber, heartRate, chooseExercise, none }
enum TimerMode { standard, tryingToReconnect, inactive }

class MainPageFinal extends StatefulWidget {
  final FlutterBlue flutterBlue = FlutterBlue.instance;
  final Firestore firestore = Firestore.instance;
  final FirebaseAuth auth = FirebaseAuth.instance;

  @override
  _MainPageFinalState createState() => _MainPageFinalState();
}

class _MainPageFinalState extends State<MainPageFinal> {
  List<BluetoothDevice> devicesList;
  BluetoothDevice _connectedDevice;
  List<BluetoothService> _cDeviceServices;
  BluetoothCharacteristic heartMeasureChar;
  bool loadingCall = false;
  bool isMeasuring = false;
  bool isTrainingSession = false;

  int currentHeartRate;
  String bandID = "None";

  StreamSubscription<List<int>> valueListener;

  AppMode currentAppMode = AppMode.inputNumber;

  String currentInput = "";
  int registeredInput = 0;

  int trainStartTime;
  int lastHeartRateTime;
  TimerMode timerMode = TimerMode.inactive;

  Timer disconnectTimer;
  Timer inputTimer;

  @override
  void initState() {
    super.initState();
    Wakelock.enable();
    print(widget.flutterBlue.connectedDevices);

    disconnectTimer = Timer.periodic(Duration(seconds: 1), (timer) {
      checkIfDisconnected();
    });

    widget.flutterBlue.scanResults.listen((results) async {
      // do something with scan results
      devicesList = new List<BluetoothDevice>();
      for (ScanResult r in results) {
        if (kCompatibleDeviceNames.contains(r.device.name)) {
          setState(() {
            devicesList.add(r.device);
          });
        }
      }
    });
  }

  Future<void> loginAnonymously() async {
    FirebaseUser user;
    while (user == null) {
      AuthResult authResult = await widget.auth.signInAnonymously();
      user = authResult.user;
    }
  }

  void checkIfDisconnected() {
    if (timerMode != TimerMode.inactive && lastHeartRateTime != null) {
      int now = DateTime.now().millisecondsSinceEpoch;
      if (timerMode == TimerMode.tryingToReconnect && (now - lastHeartRateTime > 8000)) {
        print("Reconnect didn't work in 6 secs. Repeating attempt.");
        reconnectHRMeasurer();
        lastHeartRateTime = now;
      } else if (timerMode == TimerMode.standard && (now - lastHeartRateTime > 10000)) {
        print("10 seconds without value. Attempting reconnect.");
        timerMode = TimerMode.tryingToReconnect;
        reconnectHRMeasurer();
        lastHeartRateTime = now;
      }
    }
  }

  Future<void> reconnectHRMeasurer() async {
    bool haveStarted = false;
    StreamSubscription<List<ScanResult>> scanResultsStream;
    scanResultsStream = widget.flutterBlue.scanResults.listen((results) async {
      for (ScanResult r in results) {
        if (r.device.id == _connectedDevice.id && !haveStarted) {
          print("Starting Reconnect on:" + r.device.id.toString());
          haveStarted = true;
          await _connectedDevice.disconnect();
          await r.device.connect();
          print('Device Connected');
          _cDeviceServices = await r.device.discoverServices();
          print('Services Found');
          for (BluetoothService service in _cDeviceServices) {
            if (service.uuid.toString() == kHeartRateServiceUUID) {
              for (BluetoothCharacteristic characteristic in service.characteristics) {
                if (characteristic.uuid.toString() == kHeartRateMeasureCharUUID) {
                  heartMeasureChar = characteristic;
                }
              }
            }
          }
          await heartMeasureChar.setNotifyValue(true);
          setState(() => _connectedDevice = r.device);
          scanResultsStream.cancel();
        }
      }
    });
    widget.flutterBlue.startScan(timeout: Duration(seconds: 5));
  }

  ListView _buildListViewOfDevices() {
    List<DeviceEntry> containers = new List<DeviceEntry>();
    for (BluetoothDevice device in devicesList) {
      containers.add(DeviceEntry(
        device: device,
        isConnected: device == _connectedDevice,
        isAnyConnected: _connectedDevice != null,
        onPressedConnect: () async {
          Navigator.pop(this.context);
          heartMeasureChar = null;
          startLoading();
          if (await widget.flutterBlue.isOn) {
            await device.connect();
            setState(() {
              _connectedDevice = device;
            });
            print('Device Connected');
            _cDeviceServices = await device.discoverServices();
            print('Services Found');
            for (BluetoothService service in _cDeviceServices) {
              if (service.uuid.toString() == kHeartRateServiceUUID) {
                for (BluetoothCharacteristic characteristic in service.characteristics) {
                  if (characteristic.uuid.toString() == kHeartRateMeasureCharUUID) {
                    heartMeasureChar = characteristic;
                    print('Heart Measure Service Found');
                  }
                }
              }
            }
            if (heartMeasureChar == null) {
              print("Could not detect Heart Rate Measure Char. Attempt Cancelled.");
              device.disconnect();
              endLoading();
              return;
            }

            //SET LISTENER FOR NEW VALUES OF THE MIBAND HEART RATE CHARACTERISTIC
            valueListener = heartMeasureChar.value.listen((value) {
              if (value.isNotEmpty) {
                var recentValue = value[1];
                print("Recent Value: $recentValue" + " @ " + _connectedDevice.id.toString());
                int now = DateTime.now().millisecondsSinceEpoch;
                if (recentValue != 0) {
                  widget.firestore
                      .collection('readings')
                      .document(_connectedDevice.id.toString())
                      .updateData({'rate': recentValue});

                  if (isTrainingSession)
                    widget.firestore.collection('archive').document(bandID).setData(
                        {now.toString(): (now - trainStartTime).toString() + "@" + recentValue.toString()},
                        merge: true);
                }
                currentHeartRate = recentValue;
                timerMode = TimerMode.standard;
                lastHeartRateTime = now;
                setState(() {});
              }
            });

            await heartMeasureChar.setNotifyValue(true);

            setState(() {
              timerMode = TimerMode.standard;
            });
          }
          endLoading();

          //SET BAND ID FOR MAIN SCREEN
          widget.firestore
              .collection("readings")
              .document(_connectedDevice.id.toString())
              .get()
              .then((value) {
            setState(() {
              bandID = value.data["id"];
            });
          });
        },
      ));
    }

    return ListView(
      padding: EdgeInsets.all(5),
      scrollDirection: Axis.vertical,
      shrinkWrap: true,
      children: <Widget>[
        ...containers,
      ],
    );
  }

  Widget buildBottomSheet(BuildContext context) {
    return Container(
      child: Padding(
        padding: const EdgeInsets.all(12.0),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          mainAxisAlignment: MainAxisAlignment.end,
          children: <Widget>[
            Text(
              devicesList.isNotEmpty ? 'Compatible Devices' : 'No Devices were found.',
              style: TextStyle(color: kOnBackgroundTextColor, fontSize: 24),
            ),
            _buildListViewOfDevices(),
          ],
        ),
      ),
      color: kBottomSheetColor,
    );
  }

  void sendInput(String operator) {
    if (operator == "+") registeredInput += int.parse(currentInput.substring(0, currentInput.length));
    if (operator == "=") registeredInput = int.parse(currentInput.substring(0, currentInput.length));

    print("INPUT SEND / OPERATOR: " + operator + " / VALUE: " + currentInput);
    currentInput = "";

    inputTimer.cancel();

    widget.firestore
        .collection('readings')
        .document(_connectedDevice.id.toString())
        .updateData({'input': registeredInput});

    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    return ModalProgressHUD(
      inAsyncCall: loadingCall,
      child: Scaffold(
        backgroundColor: kBackgroundColor,
        //APP BAR
        appBar: AppBar(
          centerTitle: true,
          title: Text('Master\'s Thesis: PlayHIIT',
              style: TextStyle(color: kScaffoldTitleColor, fontFamily: 'Inter', fontSize: 16)),
          backgroundColor: kScaffoldBarColor,
        ),
        body: Padding(
          padding: EdgeInsets.fromLTRB(34, 20, 34, 0),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.spaceAround,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: <Widget>[
              //FIRST TEXT
              Column(
                children: <Widget>[
                  Visibility(
                    visible: _connectedDevice == null || currentAppMode != AppMode.inputNumber,
                    child: Text(
                      'PlayHIIT Client',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        color: kOnBackgroundTextColor,
                        fontSize: 28,
                        fontFamily: 'Inter',
                      ),
                    ),
                  ),
                  Visibility(
                    visible: _connectedDevice != null && currentAppMode == AppMode.inputNumber,
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: <Widget>[
                        Text(
                          registeredInput.toString(),
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            color: kShadedBackgroundTextColor,
                            fontSize: 28,
                            fontFamily: 'Inter',
                          ),
                        ),
                        Text(
                          currentInput,
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            color: kOnBackgroundTextColor,
                            fontSize: 28,
                            fontFamily: 'Inter',
                          ),
                        ),
                        RawMaterialButton(
                          materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                          child: Icon(
                            Icons.backspace,
                            color: kIconColor,
                          ),
                          constraints: BoxConstraints.expand(width: 30, height: 30),
                          onPressed: () {
                            if (inputTimer != null) inputTimer.cancel();
                            inputTimer = Timer(Duration(seconds: 3), () {
                              sendInput("+");
                            });

                            currentInput = currentInput.substring(0, currentInput.length - 1);
                            setState(() {});
                          },
                        )
                      ],
                    ),
                  ),
                ],
              ),

              DividerColumn(),

              //SCAN BUTTON + INPUTPAD
              Flexible(
                fit: FlexFit.tight,
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: <Widget>[
                    Visibility(
                      visible: false,
                      child: Expanded(
                        child: ScanButton(
                          onPressed: () async {
                            startLoading();
                            try {
                              final result = await InternetAddress.lookup('google.com');
                              if (result.isNotEmpty && result[0].rawAddress.isNotEmpty) {
                                print('Connection status OK');
                                await loginAnonymously();
                                if (await widget.flutterBlue.isOn) {
                                  await widget.flutterBlue.startScan(timeout: Duration(seconds: 5));
                                  showModalBottomSheet(context: context, builder: buildBottomSheet);
                                } else {
                                  print("Bluetooth is turned off.");
                                }
                              }
                            } on SocketException catch (_) {
                              print('Cannot make Internet connection');
                            }
                            endLoading();
                          },
                          label: 'Scan Devices',
                        ),
                      ),
                    ),
                    Visibility(
                      visible: currentAppMode == AppMode.inputNumber,
                      child: Expanded(
                        child: InputNumpad(
                          heartRate: currentHeartRate,
                          currentInput: currentInput,
                          activeCondition: _connectedDevice != null,
                          numpadKeyOnPressed: (String s) {
                            if (currentInput == "0")
                              currentInput = currentInput.substring(0, currentInput.length - 1) + s;
                            else
                              currentInput += s;

                            if (inputTimer != null) inputTimer.cancel();
                            inputTimer = Timer(Duration(seconds: 3), () {
                              if (currentInput == "0")
                                sendInput("=");
                              else
                                sendInput("+");
                            });

                            setState(() {});
                          },
                        ),
                      ),
                    ),
                    Visibility(
                      visible: currentAppMode == AppMode.heartRate,
                      child: Expanded(
                        child: HeartRateDisplay(
                          heartRate: currentHeartRate,
                        ),
                      ),
                    ),
                  ],
                ),
              ),

              DividerColumn(),

              //BOTTOMBUTTONS
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: <Widget>[
                  //OVERWRITE BUTTON
                  Visibility(
                    visible: currentAppMode == AppMode.inputNumber,
                    child: BottomButton(
                      activeCondition:
                          _connectedDevice != null && currentInput.contains(new RegExp(r"[0-9]")),
                      iconImage: Icons.file_upload,
                      label: 'Overwrite',
                      onPressed: () {
                        sendInput("=");
                      },
                    ),
                  ),
                  //START BUTTON
                  Visibility(
                    visible: !isTrainingSession,
                    child: BottomButton(
                      activeCondition: _connectedDevice != null,
                      iconImage: Icons.play_circle_outline,
                      label: 'Start',
                      onPressed: () async {
                        isTrainingSession = true;
                        trainStartTime = DateTime.now().millisecondsSinceEpoch;
                        lastHeartRateTime = trainStartTime;

                        setState(() {});
                      },
                    ),
                  ),
                  //STOP BUTTON
                  Visibility(
                    visible: isTrainingSession,
                    child: BottomButton(
                      activeCondition: _connectedDevice != null,
                      iconImage: Icons.remove_circle_outline,
                      label: 'Stop',
                      onPressed: () async {
                        startLoading();
                        endLoading();
                        setState(() {
                          isTrainingSession = false;
                        });
                      },
                    ),
                  ),
                  //PAIR BUTTON
                  Visibility(
                    visible: _connectedDevice == null,
                    child: BottomButton(
                      activeCondition: true,
                      iconImage: Icons.bluetooth_searching,
                      label: 'Pair',
                      onPressed: () async {
                        startLoading();
                        try {
                          final result = await InternetAddress.lookup('google.com');
                          if (result.isNotEmpty && result[0].rawAddress.isNotEmpty) {
                            print('Connection status OK');
                            await loginAnonymously();
                            if (await widget.flutterBlue.isOn) {
                              await widget.flutterBlue.startScan(timeout: Duration(seconds: 5));
                              showModalBottomSheet(context: context, builder: buildBottomSheet);
                            } else {
                              print("Bluetooth is turned off.");
                            }
                          }
                        } on SocketException catch (_) {
                          print('Cannot make Internet connection');
                        }
                        endLoading();
                      },
                    ),
                  ),
                  //UNPAIR BUTTON
                  Visibility(
                    visible: _connectedDevice != null,
                    child: BottomButton(
                      activeCondition: true,
                      iconImage: Icons.bluetooth_disabled,
                      label: 'Unpair',
                      onPressed: () async {
                        startLoading();
                        await _connectedDevice.disconnect();
                        timerMode = TimerMode.inactive;
                        currentHeartRate = null;
                        lastHeartRateTime = null;
                        endLoading();

                        valueListener.cancel();

                        setState(() => _connectedDevice = null);
                      },
                    ),
                  ),
                ],
              ),
              DividerColumn(),
              //DEVICE ID TEXT
              Text(
                _connectedDevice != null
                    ? 'id: $bandID | Device: ${_connectedDevice.name}'
                    : 'No device connected.',
                textAlign: TextAlign.center,
                style: TextStyle(
                  color: kOnBackgroundTextColor,
                  fontSize: 18,
                  fontFamily: 'Inter',
                ),
              ),
              //HEART RATE TEXT
              Text(
                currentHeartRate != null
                    ? (currentHeartRate != 0
                        ? 'Current Rate: ' + currentHeartRate.toString() + ' bpm'
                        : 'No HR registered.')
                    : '',
                textAlign: TextAlign.center,
                style: TextStyle(
                  color: kOnBackgroundTextColor,
                  fontSize: 14,
                  fontFamily: 'Inter',
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  void startLoading() => setState(() => loadingCall = true);
  void endLoading() => setState(() => loadingCall = false);
}

class DividerColumn extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Column(
      children: <Widget>[
        Divider(thickness: 1, color: kOnBackgroundTextColor),
      ],
    );
  }
}

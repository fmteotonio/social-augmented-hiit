import 'package:flutter/material.dart';
import 'package:flutter_blue/flutter_blue.dart';
import '../style/entry_button.dart';
import '../style/entry_card.dart';

class DeviceEntry extends StatelessWidget {
  final BluetoothDevice device;
  final Function onPressedConnect;
  final bool isConnected;
  final bool isAnyConnected;

  DeviceEntry({this.device, this.onPressedConnect, this.isConnected, this.isAnyConnected});

  @override
  Widget build(BuildContext context) {
    return Visibility(
      visible: isAnyConnected == isConnected,
      child: EntryCard(
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.center,
          children: <Widget>[
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisAlignment: MainAxisAlignment.spaceAround,
                children: <Widget>[
                  Text(
                    'Device Name: ${device.name}',
                  ),
                  Text(
                    'Device ID: ${device.id.toString()}',
                  ),
                ],
              ),
            ),
            Visibility(
              visible: !isConnected,
              child: EntryButton(
                label: 'Connect',
                onPressed: onPressedConnect,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

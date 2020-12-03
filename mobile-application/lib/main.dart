import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'constants.dart';
import 'main_screen/main_page_final.dart';
import 'package:device_simulator/device_simulator.dart';

const bool debugEnableDeviceSimulator = false;

void main() => runApp(MyApp());

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) => MaterialApp(
        title: 'PlayHIIT',
        theme: ThemeData(textTheme: TextTheme(bodyText2: TextStyle(color: kOnBackgroundTextColor))),
        home: DeviceSimulator(
          brightness: Brightness.dark,
          enable: debugEnableDeviceSimulator,
          child: MainPageFinal(),
        ),
      );
}

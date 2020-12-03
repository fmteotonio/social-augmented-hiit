import 'package:flutter/material.dart';
import 'package:ble/constants.dart';

class ScanButton extends StatelessWidget {
  final Function onPressed;
  final String label;

  ScanButton({this.onPressed, this.label});

  @override
  Widget build(BuildContext context) {
    return RawMaterialButton(
      onPressed: onPressed,
      elevation: 10.0,
      fillColor: kScanButtonColor,
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: <Widget>[
          Icon(Icons.bluetooth_searching, color: kIconColor, size: 155),
          Text(label, style: TextStyle(fontSize: 20, color: kIconColor, fontFamily: 'Inter')),
        ],
      ),
      padding: EdgeInsets.fromLTRB(0, 30, 0, 50),
      shape: CircleBorder(),
    );
  }
}

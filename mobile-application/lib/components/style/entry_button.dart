import 'package:ble/constants.dart';
import 'package:flutter/material.dart';

class EntryButton extends StatelessWidget {
  final Function onPressed;
  final String label;

  EntryButton({this.onPressed, this.label});

  @override
  Widget build(BuildContext context) {
    return RawMaterialButton(
      elevation: 0,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8.0)),
      fillColor: kSmallButtonColor,
      child: Text(
        label,
        style: TextStyle(color: kSmallButtonTextColor),
      ),
      onPressed: onPressed,
    );
  }
}

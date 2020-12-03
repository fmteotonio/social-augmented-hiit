import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:ble/constants.dart';

class InputNumpad extends StatelessWidget {
  final int heartRate;
  final String currentInput;
  final Function numpadKeyOnPressed;
  final bool activeCondition;
  InputNumpad({this.heartRate, this.currentInput, this.numpadKeyOnPressed, this.activeCondition});

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
      children: <Widget>[
        Row(
          children: <Widget>[
            NumpadKey(onPressedFunction: numpadKeyOnPressed, text: "1", activeCondition: activeCondition),
            NumpadKey(onPressedFunction: numpadKeyOnPressed, text: "2", activeCondition: activeCondition),
            NumpadKey(onPressedFunction: numpadKeyOnPressed, text: "3", activeCondition: activeCondition),
          ],
        ),
        SizedBox(height: 4),
        Row(
          children: <Widget>[
            NumpadKey(onPressedFunction: numpadKeyOnPressed, text: "4", activeCondition: activeCondition),
            NumpadKey(onPressedFunction: numpadKeyOnPressed, text: "5", activeCondition: activeCondition),
            NumpadKey(onPressedFunction: numpadKeyOnPressed, text: "6", activeCondition: activeCondition),
          ],
        ),
        SizedBox(height: 4),
        Row(
          children: <Widget>[
            NumpadKey(onPressedFunction: numpadKeyOnPressed, text: "7", activeCondition: activeCondition),
            NumpadKey(onPressedFunction: numpadKeyOnPressed, text: "8", activeCondition: activeCondition),
            NumpadKey(onPressedFunction: numpadKeyOnPressed, text: "9", activeCondition: activeCondition),
          ],
        ),
        SizedBox(height: 4),
        Row(
          children: <Widget>[
            NumpadKey(onPressedFunction: numpadKeyOnPressed, text: "0", activeCondition: activeCondition),
          ],
        ),
      ],
    );
  }
}

class NumpadKey extends StatelessWidget {
  final Function onPressedFunction;
  final String text;
  final bool activeCondition;

  NumpadKey({this.onPressedFunction, this.text, this.activeCondition});

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: RawMaterialButton(
        constraints: BoxConstraints.expand(width: 70, height: 70),
        onPressed: () {
          if (activeCondition) this.onPressedFunction(text);
        },
        elevation: 0.0,
        fillColor: activeCondition ? kSmallButtonColor : kSmallButtonPressedColor,
        padding: EdgeInsets.fromLTRB(0, 0, 0, 0),
        shape: CircleBorder(),
        child: Text(
          this.text,
          style: TextStyle(
            fontSize: 24,
            fontFamily: "Inter",
            color: activeCondition ? kIconColor : kIconPressedColor,
          ),
        ),
      ),
    );
  }
}

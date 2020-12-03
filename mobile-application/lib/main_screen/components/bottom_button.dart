import 'package:flutter/material.dart';
import 'package:ble/constants.dart';

class BottomButton extends StatelessWidget {
  final bool activeCondition;
  final IconData iconImage;
  final String label;
  final Function onPressed;

  BottomButton({this.activeCondition, this.iconImage, this.label, this.onPressed});

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: Padding(
        padding: const EdgeInsets.fromLTRB(3, 0, 3, 0),
        child: RawMaterialButton(
          padding: EdgeInsets.all(5),
          child: Column(
            children: <Widget>[
              Icon(iconImage, color: activeCondition ? kIconColor : kIconPressedColor, size: 40),
              Text(label,
                  style: TextStyle(
                      fontSize: 16,
                      fontFamily: 'Inter',
                      color: activeCondition ? kIconColor : kIconPressedColor)),
            ],
          ),
          onPressed: activeCondition ? onPressed : null,
          fillColor: activeCondition ? kSmallButtonColor : kSmallButtonPressedColor,
          elevation: activeCondition ? 10 : 0,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8.0)),
        ),
      ),
    );
  }
}

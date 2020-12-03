import 'package:flutter/material.dart';
import 'package:ble/constants.dart';

class HeartRateDisplay extends StatelessWidget {
  final int heartRate;

  HeartRateDisplay({this.heartRate});

  @override
  Widget build(BuildContext context) {
    return RawMaterialButton(
      onPressed: () {},
      elevation: 0.0,
      fillColor: kHRDisplayColor,
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: <Widget>[
          Text(
            'Current Rate: ',
            style: TextStyle(fontSize: 20, color: kIconColor, fontFamily: 'Inter'),
          ),
          Row(
            crossAxisAlignment: CrossAxisAlignment.baseline,
            mainAxisAlignment: MainAxisAlignment.center,
            textBaseline: TextBaseline.alphabetic,
            children: <Widget>[
              Text(
                heartRate != null ? heartRate.toString() : '--',
                style: TextStyle(fontSize: 100, color: kIconColor, fontFamily: 'Inter'),
              ),
              Text(
                'bpm',
                style: TextStyle(fontSize: 20, color: kIconColor, fontFamily: 'Inter'),
              ),
            ],
          ),
          Icon(
            Icons.favorite,
            color: kIconColor,
            size: 60,
          )
        ],
      ),
      padding: EdgeInsets.fromLTRB(0, 40, 0, 20),
      shape: CircleBorder(),
    );
  }
}

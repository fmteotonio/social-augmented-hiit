import 'package:ble/constants.dart';
import 'package:flutter/material.dart';

class EntryCard extends StatelessWidget {
  final Widget child;

  EntryCard({this.child});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: EdgeInsets.all(3),
      decoration: BoxDecoration(
        color: kEntryCardColor,
        borderRadius: BorderRadius.all(
          Radius.circular(10),
        ),
      ),
      height: 50,
      child: Padding(
        padding: const EdgeInsets.all(8.0),
        child: child,
      ),
    );
  }
}

import 'package:flutter/material.dart';
import '../style/entry_button.dart';
import '../style/entry_card.dart';

class ModeEntry extends StatelessWidget {
  final Function onPressed;
  final String name;
  final String description;

  ModeEntry({this.name, this.description, this.onPressed});

  @override
  Widget build(BuildContext context) {
    return EntryCard(
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: <Widget>[
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisAlignment: MainAxisAlignment.spaceAround,
              children: <Widget>[
                Text(
                  name,
                  style: TextStyle(fontWeight: FontWeight.w800),
                ),
                Text(
                  description,
                  style: TextStyle(fontSize: 8),
                ),
              ],
            ),
          ),
          EntryButton(
            label: 'Select',
            onPressed: onPressed,
          ),
        ],
      ),
    );
  }
}

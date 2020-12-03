import 'package:flutter/material.dart';

enum MeasureMode { continuous, instantaneous, both }

/*------------Change Device names and UUID's here accordingly------------------*/
const List<String> kCompatibleDeviceNames = ['Mi Band 3'];
const String kHeartRateServiceUUID = '0000180d-0000-1000-8000-00805f9b34fb';
const String kHeartRateMeasureCharUUID = '00002a37-0000-1000-8000-00805f9b34fb';

Color black = Colors.black;
Color white = Colors.white;

Color kColorNumber4 = Color(0xff0F5298);
Color kColorNumber3 = Color(0xff3C99DC);
Color kColorNumber2 = Color(0xff66D3FA);
Color kColorNumber1 = Color(0xffD5F3FE);

Color kScaffoldBarColor = kColorNumber4;
Color kScaffoldTitleColor = white;

Color kBackgroundColor = black;
Color kOnBackgroundTextColor = kColorNumber1;
Color kIconColor = white;
Color kIconPressedColor = kColorNumber3;

Color kBottomSheetColor = black;

Color kEntryCardColor = kColorNumber4;
Color kSmallButtonColor = kColorNumber3; //3
Color kSmallButtonPressedColor = kColorNumber4; //2
Color kSmallButtonTextColor = white;

Color kScanButtonColor = kColorNumber3; //3
Color kHRDisplayColor = kColorNumber4; //2

Color kShadedBackgroundTextColor = kColorNumber4;

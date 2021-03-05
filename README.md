# PlayHIIT

PlayHIIT is a system designed to complement **videoconference-based group exercise sessions**. It is composed by a mobile and a desktop application. 

The **desktop application** displays a shared screen between all of the participating users, that displays both individual and group performance data to everyone, while providing relevant information through visual and audio that is connected to different events of the activity such as someone increasing their heart rate in a substantial way.

The **mobile application** is used to connect to an Activity Tracking Band (ATB) to gather heart rate, as well as providing a repetition counter for users to wield, sending all this data to a cloud stored database where the desktop application gets its required information from.

![ApparatusSharedScreen](https://user-images.githubusercontent.com/63672636/110160852-79897880-7de4-11eb-9981-ab347b119072.PNG)


# License

- The following software is licensed under Creative Commons Zero v1.0 Universal
- SimpleJSON is used under MIT License featuring due copyright notice.
- Images used under Freepik license from the following sources:
  - https://www.freepik.com/free-vector/transportation-icons_1188955.htm
  - https://www.freepik.com/free-vector/package-box-symbols-set_9650604.htm

# Info

- The project is setup to work with the Xiaomi Mi Band 3 band.
- Additional bands may be supported by adding them in the mobile application in the lib/main_screen/constants.dart file, on line 4, although no other models were tested.

# Setup Instructions

## Setting the database up

- Setup a Firebase project and add it to the mobile application.
  - Use the following guide: https://firebase.google.com/docs/android/setup
  - Follow Option 1, Steps 1 to 3. 
- Setup a Cloud Firestore database through the Firebase console:
  - The suggested structure for the database is as follows:
    - Collection named "readings" contains:
      - One document per band named with its MAC address containing:
        - string value "MAC"      : MAC Adress.
        - string value "id"       : User ID to be displayed on both applications.
        - number value "input"    : Default to 0.
        - number value "max-rate" : Maximum heart rate of the user.
        - number value "rate"     : Default to 0.
- Turn on Anonymous Authentication Sign-In Method
- Provide database with security rules to allow public read.
```
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    match /{document=**} {
      allow read, write: if request.auth != null;
      allow read: if true;
    }
  }
}
```

## Mobile Application

### android/app/build.gradle

- In line 42, specify unique Application ID. (https://developer.android.com/studio/build/application-id.html)

## Desktop Application

### FirestorAPIControllerScript.cs 
- In line 39, exchange the asterisks for the name of the Firebase project in which you have your database associated.
- Optionally, in Unity Scene Editor, under SampleScene/PlayerGroup/PlayerVoiceAudioSource replace Missing Audio Clips with audio files of choice.

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

## Mobile Application

### android/app/build.gradle

- In line 42, specify unique Application ID. (https://developer.android.com/studio/build/application-id.html)

## Desktop Application

### FirestorAPIControllerScript.cs 
- In line 39, exchange the asterisks for the name of the Firebase project in which you have your database associated.
- Optionally, in Unity Scene Editor, under SampleScene/PlayerGroup/PlayerVoiceAudioSource replace Missing Audio Clips with audio files of choice.

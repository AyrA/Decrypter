# Decrypter

Decrypts DLC files using a web service

## What it does

This application decrypts DLC files using a web service. That magic happens in `ManualUpload.cs`

## Installing

1. Put the exe and dll file into a directory
1. Right-click on a DLC file and select "Open With"
1. Browse for the exe file

## Using

### By installing

After you did the "Installing" chapter you can just double click on any dlc file and the application will present you with the decrypted links (if any).

### Portable

You can just double click the exe file and then browse for the DLC manually if you want to.

## Auto-save

You can provide a second command line argument.
If you do so the application will save all decrypted links to the given file, overwriting any existing copy.

## Always online

DLC files (by design) require a webservice to be decrypted.
This means you can't use them offline.
This is not a limitation of this application but an intended feature of DLC files.

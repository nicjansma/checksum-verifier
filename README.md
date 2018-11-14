# ChecksumVerifier

v1.3.0

Copyright 2018 Nic Jansma

[http://nicj.net](http://nicj.net)

Licensed under the MIT license

## Introduction

ChecksumVerifier is a Windows command-line tool that helps you validate the integrity of your files.  ChecksumVerifier
maintains a database of files and their [checksums](http://en.wikipedia.org/wiki/Checksum), and lets you validate that
their contents have not been changed.

A [checksum](http://en.wikipedia.org/wiki/Checksum) is a small sequence of 20-200 characters (depending on the specific
algorithm used) that is calculated by reading the input file and applying a mathematical algorithm to its
contents.  Even a file as large as 1TB will only have a small 20-200 character checksum, so checksums are an efficient
way of saving the file's state without saving it's entire contents.  ChecksumVerifier uses the
[MD5](http://en.wikipedia.org/wiki/MD5), [SHA-1](http://en.wikipedia.org/wiki/SHA-1),
[SHA-256](http://en.wikipedia.org/wiki/SHA-256) and [SHA-512](http://en.wikipedia.org/wiki/SHA-512) algorithms,
which are generally [collision resistant](http://en.wikipedia.org/wiki/Collision_resistant) enough for validating
the integrity of file contents.

One [example](#examples) usage of ChecksumVerifier is to verify the integrity of external hard drive backups.  After saving files
to an external disk, you can run `ChecksumVerifier -update` to calculate the checksums of all of the files on the
external disk.  At a later date, if you want to validate that the files on the disk have not been added, removed or changed,
you can run `ChecksumVerifier -verify` and it will re-calculate all of the disks' checksums and compare them to the original
database to see if any files have been changed in any way.

## Usage

Running `ChecksumVerifier -help` will print out the program's help screen:

```
Usage: ChecksumVerifier.exe [-update | -verify] -db [xml file] [options]

actions:
     -update:                Update checksum database
     -verify:                Verify checksum database

required:
     -db [xml file]          XML database file

options:
     -match [match]          Files to match (glob pattern such as * or *.jpg or ??.foo) (default: *)
     -exclude [match]        Files to exclude (glob pattern such as * or *.jpg or ??.foo) (default: empty)
     -basePath [path]        Base path for matching (default: current directory)
     -r, -recurse            Recurse (directories only, default: off)

path storage options:
     -relativePath           Relative path (default)
     -fullPath               Full path
     -fullPathNodrive        Full path - no drive letter

checksum options:
     -md5                    MD5 (default)
     -sha1                   SHA-1
     -sha256                 SHA-2 256 bits
     -sha512                 SHA-2 512 bits

-verify options:
     -ignoreMissing          Ignore missing files (default: off)
     -showNew                Show new files (default: off)
     -ignoreChecksum         Don't calculate checksum (default: off)

-update options:
     -removeMissing          Remove missing files (default: off)
     -ignoreNew              Don't add new files (default: off)
     -updateExisting         Verifies the Checksums of all existing files and updates the database on changes (default: off)
     -pretend                Show what would happen - don't write out XML (default: off)
 ```

### Actions

The two main actions are `-update` and `-verify`.

#### `-update`

Scans for files, calculates their checksums and saves the results to the XML file.

`-update` options:

##### `-removeMissing`

Default: `off`

If set, if files in the database are no longer on disk, they will be removed.

##### `-ignoreNew`

Default: `off`

If set, new files on the disk will not be added to the database.

##### `-updateExisting`

Default: `off`

If set, verifies the Checksums of all existing files and updates the database on changes.

##### `-pretend`

Default: `off`

If set, files will be scanned and checksums will be calculated, but the results will not be saved to the database.

#### `-verify`

Verifies the checksums of all the files listed in the database.  If there are any missing files, incorrect checksums,
or new files (if `-showNew` is set), they will be listed in the program output and the program will exit with a
non-zero error code.

If the error code is -1, there was an error in running the `-verify` command, such as if the XML file was not found.

If the error code is greater than 0, it is the number of files that were missing, had incorrect checksums, or
were new files (if `-showNew` is set).

`-verify` options:

##### `-ignoreMissing`

Default: `off`

If set, ignores when files are listed in the datbaase, but are no longer on disk.

##### `-showNew`

Default: `off`

If set, shows new files that are on disk but are not listed in the database.

##### `-ignoreChecksum`

Default: `off`

If set, does not calculate the checksums of files (only looks for missing and/or new files).

### General Options

#### Required

##### `-db [xml file]`

ChecksumVerifier stores its database of files and their checksums in an XML file.  `-db` must be used to specify
the location of the XML file. The XML file can be stored separately from the actual files.

#### Miscellaneous options

##### `-match [match]`

Default: `*`

The pattern of files to match.

Must be a DOS glob pattern, such as `*` or `*.jpg` or `??.foo`.

##### `-exclude [match]`

Default: (empty)

Files to exclude.

Must be a DOS glob pattern, such as `*` or `*.jpg` or `??.foo`.

##### `-basePath [path]`

Default: Current directory

The base path to use when scanning for files.

##### `-r` or `-recurse`

Default: `off`

If set, scans for files in all folders underneath the `-basePath` recursively.

#### Path Storage Options

These options modify the way a file's name is stored in the XML database.

The default path storage option is `-relativePath`.

##### `-relativePath`

This is the default option.

Files will be stored with their name relative to their `-basePath`.

For example, using  `-basePath C:\folder`, a file with a path of `C:\folder\sub\file.ext` will be saved
as `sub\file.ext`.

##### `-fullPath`

Files will be stored with their full path.

For example, using  `-basePath C:\folder`, a file with a path of `C:\folder\sub\file.ext` will be saved
as `C:\folder\sub\file.ext`.

This option could be used if you want to store files located on multiple disks in the same database.

##### `-fullPathNodrive`

Files will be stored with their full path minus the drive letter.

For example, using  `-basePath C:\folder`, a file with a path of `C:\folder\sub\file.ext` will be saved
as `folder\sub\file.ext`.

This option could be used if you want to selectively store files that are located on a removable drive whose drive letter may
change.  You would later use the `-basePath` option and specify the new drive letter.

#### Checksum Options

These options change which checksum algorithm is used.  Each algorithm has a different digest size, which is the number
of bits in the calculated checksum.  The more bits, the *more unlikely* it is that two random files with different
contents will have the same checksum.  In other words, the more bits, the *higher confidence* you will have that your
file has not changed if the checksum is the same as it was before.

On the other hand, the higher the digest size, the longer it takes to calculate, and the more storage (bytes) are used
in the database XML file.

The default checksum is `-md5`.

##### `-md5`

Use the [MD5](http://en.wikipedia.org/wiki/MD5) checksum algorithm.  128 bit digest.

##### `-sha1`

Use the [SHA-1](http://en.wikipedia.org/wiki/SHA-1) checksum algorithm.  160 bit digest.

##### `-sha256`

Use the [SHA-256](http://en.wikipedia.org/wiki/SHA-256) checksum algorithm.  256 bit digest.

##### `-sha512`

Use the [SHA-512](http://en.wikipedia.org/wiki/SHA-512) checksum algorithm.  512 bit digest.

## Examples <a name="examples"></a>

### Verifying Files Saved to an External Hard Drive

Scenario: You have thousands of photos that you want to save to an external hard drive so you can ship it to a friend
who lives in another country.  You want to ensure that when they get the drive, they can validate that the files have
not been changed or corrupted during shipping.

Here are the steps you would take:

1. Save all of the files to the external hard disk via any method you want (Windows File Explorer, `xcopy`, `robocopy`,
    etc).

2. From the command prompt, `cd E:`, where `E:` is your external hard drive's disk letter.

3. Run the following `-update` command to generate the ChecksumVerifier database:

````
ChecksumVerifier.exe -db photos.xml -update -recurse
````

4. After the `files.xml` database has been generated, you could copy the database to your computer for safe-keeping,
    then un-mount the disk and send it to your friend.

5. Once your friend receives the disk, they can run `-verify` to ensure that no files were added, removed or changed.
    You could even email them your copy of `files.xml` to ensure their version was not tampered with.

````
ChecksumVerifier.exe -db photos.xml -verify -shownew
````

6. If any files were added, removed or changed, the program will list them and return a non-zero exit code.

## Version History

* v1.3.0 - 2018-11-14: Supports Long File Path
* v1.2.0 - 2018-05-09: Don't set cursor if output is redirected
* v1.1.0 - 2018-04-21: Added `-updateExisting` update option
* v1.0.0 - 2014-02-02: Initial version

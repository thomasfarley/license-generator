# license-generator
A class library for randomly generating license codes. .NET Standard

## Formatting

* <b>N => Random number 0-9</b>
* <b>C => Random capital letter A-Z</b>
*  <b>c => Random lowercase letter a-z</b>

##### Any other character is acceptable in the formatting to make your generated codes more unique. Only the above three will be parsed out of the format.

## Usage
```csharp
// Reference
using Indygo.License;

// Initialize the generator
var gen = new Generator();

// Set your desired format
gen.SetFormat("CCCCC-NNNNN-CCCCC-CcCcC");

// You can choose choose what file format you want to save your licenses in. By default, CSV will be chosen.
public enum FileFormat { CSV, NewLine, Delimeter, JSON, Excel }
// You can specify a custom delimeter as well. Choose Delimeter as your FileFormat and there will be an 
// additional parameter in the method called delimeter that is set to a comma by default.

// Generate keys
gen.Generate(int count, FileFormat saveFormat, string fileDirectory);
// Returns: List<string>
```

Some features are not implemented at this time. This is a work in progress.

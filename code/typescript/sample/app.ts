import {CremaDataSet} from "./crema-code-tables";

let dataSet: CremaDataSet = CremaDataSet.createFromFile("./crema.dat", false);
console.log(dataSet.name);
console.log(dataSet.revision);
console.log(dataSet.tags);
console.log(dataSet.typesHashValue);
console.log(dataSet.tablesHashValue);

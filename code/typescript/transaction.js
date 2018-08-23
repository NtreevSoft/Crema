/// <reference path="./crema.d.ts" />
var dataBaseName = "master";
var token = login("admin", "admin");
if (isDataBaseLoaded(dataBaseName) === false) {
    loadDataBase(dataBaseName);
}
if (isDataBaseEntered(dataBaseName) === false) {
    enterDataBase(dataBaseName);
}
beginTableCreate(dataBaseName, "/");
var transactionID = beginDataBaseTransaction(dataBaseName);
try {
    beginTableCreate(dataBaseName, "/");
    cancelDataBaseTransaction(transactionID);
}
finally {
    leaveDataBase("master");
    logout(token);
}

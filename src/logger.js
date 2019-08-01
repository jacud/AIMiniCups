var fs = require('fs');

class Logger {

  constructor(fileName) {
    this.fileName = fileName;
	this.logs = []
  }

  log(obj) {
    this.logs.push(obj);
	this.logs.push("\r\n");	
  }
  
  SaveLog() {
	fs.writeFile (this.fileName, JSON.stringify(this.logs), function(err) {})
  }
}

export default Logger;

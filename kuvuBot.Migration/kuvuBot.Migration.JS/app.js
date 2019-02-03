"use strict";
exports.__esModule = true;
var r = require("rethinkdb");
var fs = require("fs");
var args = process.argv.slice(2);
console.log("Connecting to " + args[0] + ":" + args[1] + ", as " + args[2] + ", database " + args[4]);
var connection;
r.connect({ host: args[0], port: +args[1], user: args[2], password: args[3], db: args[4] }, function (err, conn) {
    if (err)
        throw err;
    connection = conn;
    r.table('guilds').run(connection, function (err, cursor) {
        if (err)
            throw err;
        cursor.toArray(function (err, result) {
            if (err)
                throw err;
            fs.writeFile("out.json", JSON.stringify(result, null, 2), function (err) {
                if (err) {
                    return console.log(err);
                }
                console.log("The guilds table was scrapped to out.json!");
            });
        });
    });
});

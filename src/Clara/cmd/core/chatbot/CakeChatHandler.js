/**
 * @file CakeChatHandler.js
 * @description A Handler module to handle CakeChat responses
 * @author Capuccino
 * @license MIT
*/
 
const https = require('https');
const URL = require('url');
const HostRegex = str => /^([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])\
(\.([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9]))*$/gi.test(str);

/**
  * Class that exposes all the methods to query to a cakechat instance
  * @class
*/
class CakeChatHandler {
    /**
     * 
     * @param {String} url the URL for your instance. Must be a valid IP or hostname. 
     */
    constructor(url) {
        this.url = url;
        if(!HostRegex(url)) return new TypeError('url is not a valid hostname/IP');
        if(typeof url !== 'string') return new TypeError('url is not a string.');
    }

    /**
     * Gets Response from CakeChat Server
     * @param {Array} context the context to pass where the second index in the array is the current input
     * @returns {Promise<Object>} the response text.
     */
    getResponse(context) {
        return new Promise((resolve, reject) => {
            if (!Array.isArray(context)) return reject('context is not an array.');
            else resolve(request('POST', `${this.url}/cakechat_api/v1/actions/get_response`, {
                headers: {
                    'Content-Type': 'application/json',
                    'User-Agent': 'Clara/0.3.0-Monika'
                }

            }, JSON.stringify({
                context: context,
                from_cakechat: Boolean(true),
            })));
        });
    }
}

// simple request function for creating a Promisified HTTP/S request.
function request(method, url, options={}, payload) {
    return new Promise((resolve, reject) => {
        let req = https.request(Object.assign(URL.parse(url), options, {method}), res => {
            let chunked = '';

            if (res.statusCode !== 200) return reject(new Error(`HTTP ${res.statusCode}: ${res.statusMessage}`));

            res.setEncoding('utf8');
            res.on('data', chunk => chunked += chunk);
            res.on('error', reject);
            res.on('end', () => {
                let val;

                try {
                    val = JSON.parse(chunked);
                } catch(e) {
                    return resolve(chunked);
                }

                if (val.error) return reject(new Error(val.error));
                else return resolve(val);
            });
        });

        req.on('error', reject);
        if (payload) req.write(payload);
        req.end();
    });
}

module.exports = CakeChatHandler;
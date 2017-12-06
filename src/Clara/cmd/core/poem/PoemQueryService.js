/**
 * @file query handler for Poem Service for Remote type of connections for Sayori. 
 * @author Capuccino
 */
 
 const got = require('got');
 
 /**
  * Query wrapper for the Python service
  * @class
  */
 class PoemService {
     /**
      * @param {String} serviceHost the service's address. Must be valid HTTP/HTTPS adress.
      */ 
     constructor(serviceHost) {
         this.serviceHost = serviceHost;
         let serviceHostRegex = /^(?:(?:https?:)?\/\/)?(?:\S+(?::\S*)?@)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})))(?::\d{2,5})?(?:[/?#]\S*)?$/i;
       
         // check if serviceHost parameter is valid HTTP/HTTPS address
         if(!serviceHostRegex.test(serviceHost)) throw new Error('serviceHost is not a valid HTTP/HTTPS address');
     }
     
     /**
      * Generates a poem based from a input
      * @param {String} font the font to use. Acceptable inputs are documented at https://github.com/MonikaDesu/Sayori
      * @param {String} content the content.
      * @returns {Promise<Object>} The response from the API.
      */
     generatePoem(font, content) {
         return new Promise((resolve, reject) => {
             got(`${this.serviceHost}/generate`, {
                 //force encoding to utf-8
                 encoding:'utf8',
                 method: 'POST',
                 json: true,
                 body: {
                     poem: content,
                     font
                 }
             })
             .then(res => {
                 resolve(res.body);
             })
             .catch(err => {
                 reject(err);
             });
         });
     }
 }
 
 module.exports = PoemService;
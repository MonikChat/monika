/**
 * @file query handler for Poem Service for Remote type of connections for Sayori. 
 * @author Capuccino
 */

const got = require('got');

/**
 * Simple handler for the [Sayori]{@link https://github.com/MonikaDesu/Sayori/} poem generator service.
 */
class PoemHandler {
    /**
     * Creates a new PoemHandler.
     * 
     * @param {String} [host='127.0.0.1'] Host which the poem server is running under.
    */ 
    constructor(host) {
        host = host || 'http://127.0.0.1:8080';

        if (typeof host !== 'string') throw new TypeError('host is not a string.');

        this.host = host;
    }
    
    /**
     * Generates a poem based from a input
     * @param {String} content Text content of the poem.
     * @param {String} [font='m1'] Font to generate the poem with. Supported fonts can be found [here]{@link https://github.com/MonikaDesu/Sayori/blob/master/API.md#supported-fonts}
     * @returns {Promise<String>} Generated poem image.
     */
    generatePoem(content, font='m1') {
        return new Promise((resolve, reject) => {
            got.post(`${this.host}/generate`, {
                encoding: null,
                body: JSON.stringify({
                    poem: content,
                    font
                })
            }).then(res => resolve(res.body)).catch(reject);
        });
    }
}

module.exports = PoemHandler;
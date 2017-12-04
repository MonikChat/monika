/**
 * @file Chatbot using Monika.Dialogflow session.
 * @author Capuccino
 * @author Ovyerus
 * @author FiniteReality
 */

const dialogflow = require('dialogflow');
const uuid = require('uuid/v4');
let client;
let spPath;
exports.commands = ['chat'];

exports.init = bot => {
    client = new dialogflow.SessionsClient();
    // @FiniteReality requested to use random UUIDs instead
    spPath = client.sessionPath(uuid.uuidv4(), bot.config.dialogFlowProjectID);
};


exports.chat = {
    desc: 'Chat to Monika',
    usage: '<message>',
    async main(bot, ctx) {
        if (!ctx.suffix) {
            let result = await client.detectIntent(__createResponseObject('hey', 'en'));
            await ctx.createMessage(result[0].queryResult.queryText);
        } else {
            let result = await client.detectIntent(__createResponseObject(ctx.createMessage, 'en'));
            await ctx.createMessage(result[0].queryResult.queryText);
        }
    }
};


/**
 * Creates a response object for Dialogflow.
 * 
 * @param {String} res Response to send
 * @param {String} [lang='en'] Language the response is in. Defaults to en
 * @internal
 * @returns {Object} .
 */
function __createResponseObject(res, lang='en') {
    return {
        session: spPath,
        queryInput: {
            text: res,
            languageCode: lang || 'en'
        }
    };
}
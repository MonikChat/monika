/**
 * @file Chatbot using Monika.Dialogflow session.
 * @author Capuccino
 * @author Ovyerus
 * @author FiniteReality
 */

const dialogflow = require('dialogflow');
let client;
let spPath;
exports.commands = ['chat'];

exports.init = () => {
    client = new dialogflow.SessionsClient();
    spPath = client.sessionPath(config.dialogFlowSessionID, dialogFlowProjectID);
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
 * creates a Response object for Dialogflow
 * @param {String} res the response to send
 * @param {String} lang the language the response is. Defaults to en
 * @internal
 */
function __createResponseObject(res, lang='en') {
    return {
        session: spPath,
        queryInput: {
            text: res,
            languageCode: lang || 'en'
        },
    };
}
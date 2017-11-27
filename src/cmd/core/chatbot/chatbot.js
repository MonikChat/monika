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
    main(bot, ctx) {
        if (!ctx.suffix) {
         await client.detectIntent(__createResponseObject('hey', 'en'))
            .then(res => {
                const result = res[0].queryResult;
                await ctx.createMessage(result.queryText);
            }).catch(reject);
        } else {
            await client.detectIntent(__createResponseObject(ctx.cleanSuffix, 'en'))
            .then(res => {
                const result = res[0].queryResult;
                await ctx.createMessage(result.queryText);
            }).catch(reject);
        }
    }
};


/**
 * creates a Response object for Dialogflow
 * @param {String} res the response to send
 * @param {String} lang the language the response is. Defaults to en
 * @internal
 */
function __createResponseObject(res, lang) {
    return {
        session: spPath,
        queryInput: {
            text: res,
            languageCode: lang || 'en'
        },
    };
}
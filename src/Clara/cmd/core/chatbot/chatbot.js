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
    if (!bot.config.dialogFlowProjectID || !typeof bot.config.dialogFlowProjectID === 'string') throw new Error('dialogFlowProjectID is not a string');
    else spPath = client.sessionPath(uuid.uuidv4(), bot.config.dialogFlowProjectID);
};


exports.chat = {
    desc: 'Chat to Monika',
    usage: '<message>',
    async main(bot, ctx) {
        if(!ctx.suffix) {
            await ctx.createMessage('Ehehe~, What is it?');
        } else {
            //TODO : Response Object Model for Rebecca
            throw new Error('not implemented.');
        }
    }
};


/**
 * @file Chatbot using Monika.Dialogflow session.
 * @author Capuccino
 * @author Ovyerus
 * @author FiniteReality
 */

const responses = require('./emptyResponseDialogues.json');
exports.commands = ['chat'];

exports.chat = {
    desc: 'Chat to Monika',
    usage: '<message>',
    async main(bot, ctx) {
        if (!ctx.suffix) {
            let dialogue = responses[Math.floor(Math.random() * responses.length)];
            await ctx.createMessage(dialogue);
        } else {
            //TODO : Response Object Model for Rebecca
            throw new Error('not implemented.');
        }
    }
};


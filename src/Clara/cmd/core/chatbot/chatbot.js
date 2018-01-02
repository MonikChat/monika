/**
 * @file Chatbot using Monika.Dialogflow session.
 * @author Capuccino
 * @author Ovyerus
 * @author FiniteReality
 */

exports.commands = ['chat'];

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


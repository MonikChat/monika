/**
 * @file Chatbot using Monika.Dialogflow session.
 * @author Capuccino
 * @author Ovyerus
 * @author FiniteReality
 */

const responses = require('./emptyResponseDialogues.json');
const CakeChat = require('./CakeChatHandler');
let cb;

exports.init = bot => {
    if (!bot.config.cakeChatInstanceURL && !process.env.CAKECHAT_URL) throw new Error('CakeChat URL not found from config or env vars.');

    cb = new CakeChat(process.env.CAKECHAT_URL || bot.config.cakeChatInstanceURL);
};

exports.commands = ['chat'];

exports.chat = {
    desc: 'Chat to Monika',
    usage: '<message>',
    async main(bot, ctx) {
        if (!ctx.suffix) {
            let dialogue = responses[Math.floor(Math.random() * responses.length)];
            return await ctx.createMessage(dialogue);
        }

        await ctx.channel.sendTyping();

        let response = await cb.getResponse([ctx.suffix]);
        await ctx.createMessage(response);
    }
};


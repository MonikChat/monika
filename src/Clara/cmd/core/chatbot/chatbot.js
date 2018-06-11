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
    if(!bot.config.cakeChatInstanceURL) cb = new CakeChat(process.env.CAKECHAT_URL);
    else if (!process.env.CAKECHAT_URL) return new Error('CakeChat URL not found from config or env vars.');
    else cb = new CakeChat(bot.config.cakeChatInstanceURL);
};
exports.commands = ['chat'];

exports.chat = {
    desc: 'Chat to Monika',
    usage: '<message>',
    async main(bot, ctx) {
        if (!ctx.suffix) {
            let dialogue = responses[Math.floor(Math.random() * responses.length)];
            await ctx.createMessage(dialogue);
        } else {
            cb.getResponse([ctx.suffix]).then(r => {
                let resp = JSON.parse(r.body.response);
                ctx.createMessage(resp);
            });
        }
    }
};


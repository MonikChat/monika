/**
 * @file directMessage.js
 * @description Event to allow handling of Personal Messaging and resolves to chatbot command module
 * @author Capuccino
 */
const path = require('path');
const {Context} = require(path.resolve(__dirname, '../modules', 'CommandHolder'));

module.exports = bot => {
    bot.on('messageCreate', async m => {
        if (!m.channel.guild || m.author.id !== bot.user.id || bot.isBlacklisted(m.author.id)) {
            try {
                let ctx = new Context(m, bot);
                ctx.cmd = 'chat';
                bot.commands.runCommand(ctx);
            } catch(e) {
                logger.error(e);
            }
        }
    });
};
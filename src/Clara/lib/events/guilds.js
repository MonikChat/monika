/**
 * @file Guild events file.
 * @author Capuccino
 * @author Ovyerus
 */

module.exports = bot => {
    bot.on('guildCreate', async guild => {
        if (guild.members.filter(m => m.bot).filter / guild.members.size >= 0.50) {
            logger.info(`Detected bot collection guild '${guild.name}' (${guild.id}). Autoleaving...`);
            await guild.leave();
        } else if (!bot.config.url && bot.config.gameURL) {
            await bot.editStatus('online', {
                name: `${bot.config.gameName || `${bot.config.mainPrefix}help for commands!`} | ${bot.guilds.size} ${bot.guilds.size === 1 ? 'server' : 'servers'}`,
                type: 0,
                url: null
            });
        } else {
            await bot.editStatus('online', {
                name: `${bot.config.gameName || `${bot.config.mainPrefix}help for commands!`} | ${bot.guilds.size} ${bot.guilds.size === 1 ? 'server' : 'servers'}`,
                type: 1,
                url: bot.config.gameURL
            });
        }
    });

    bot.on('guildDelete', async guild => {
        if (guild.members.filter(m => m.bot).filter / guild.members.size >= 0.50) return;

        if (!bot.config.url && bot.config.gameURL) {
            await bot.editStatus('online', {
                name: `${bot.config.gameName || `${bot.config.mainPrefix}help for commands!`} | ${bot.guilds.size} ${bot.guilds.size === 1 ? 'server' : 'servers'}`,
                type: 0,
                url: null
            });
        } else {
            await bot.editStatus('online', {
                name: `${bot.config.gameName || `${bot.config.mainPrefix}help for commands!`} | ${bot.guilds.size} ${bot.guilds.size === 1 ? 'server' : 'servers'}`,
                type: 1,
                url: bot.config.gameURL
            });
        }

        await bot.postGuildCount(); 
    });
};

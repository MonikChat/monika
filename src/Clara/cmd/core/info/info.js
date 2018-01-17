/**
 * @file Various information of the bot.
 * @author Ovyerus
 */

const fs = require('fs');
const path = require('path');
var version;

try {
    version = JSON.parse(fs.readFileSync(path.resolve(`${mainDir}`, '../../../', './package.json'))).version;
} catch(_) {
    version = JSON.parse(fs.readFileSync(path.resolve(`${mainDir}`, './package.json'))).version;
}

exports.commands = [
    'info'
];

exports.info = {
    desc: 'Information about Monika.',
    owner: true,
    async main(bot, ctx) {
        let roleColour = ctx.guildBot.roles.sort((a, b) => ctx.guild.roles.get(b).position - ctx.guild.roles.get(a).position)[0];
        roleColour = roleColour ? ctx.guild.roles.get(roleColour).color : 0;

        await ctx.createMessage({embed: {
            title: `${bot.user.username}'s Info`,
            description: '[Source Code~](https://github.com/sr229/monika) | [Support Server~](https://discord.gg/rmMTZue)',
            thumbnail: {url: bot.user.avatarURL},
            footer: {text: 'Just Monika'},
            color: roleColour,
            fields: [
                {
                    name: 'Clubs',
                    value: bot.guilds.size,
                    inline: true
                },
                {
                    name: 'Members',
                    value: bot.users.size,
                    inline: true
                },
                {
                    name: 'Game Time',
                    value: utils.msToTime(bot.uptime),
                    inline: true
                },
                {
                    name: 'Hearts',
                    value: bot.shards.size,
                    inline: true
                },
                {
                    name: 'Memory',
                    value: utils.genBytes(process.memoryUsage().rss),
                    inline: true
                },
                {
                    name: 'Version',
                    value: version, inline:
                    true
                }
            ]
        }});
    }
};
/**
 * @file Dynamic blacklist command.
 * @author Ovyerus
 */

const fs = require('fs');

exports.loadAsSubcommands = true;
exports.commands = [
    'add',
    'remove'
];

exports.main = {
    desc: 'Add or remove people in the blacklist.',
    usage: '[<add|remove> <mention|id>]',
    owner: true,
    async main(bot, ctx) {
        let embed = {
            title: 'Ignored Users',
            description: []
        };

        if (bot.blacklist.length === 0) {
            embed.description = 'You did not ask me to ignore anyone, Anon~!';
            return await ctx.createMessage({embed});
        }

        bot.blacklist.forEach(u => {
            if (bot.users.get(u)) {
                embed.description.push(`**${utils.formatUsername(bot.users.get(u))}** (${u})`);
            } else {
                embed.description.push(`**Unknown user** (${u})`);
            }
        });

        embed.description = embed.description.join('\n');

        await ctx.createMessage({embed});
    }
};

exports.add = {
    desc: 'Add a user to the blacklist.',
    usage: '<mention|id>',
    async main(bot, ctx) {
        if (!/^(<@!?\d+>|\d+)$/.test(ctx.args[0])) return await ctx.createMessage('Please mention the user to ignore, or their id.');
        let id = /^<@!?\d+>$/.test(ctx.args[0]) ? ctx.args[0].replace(/^<@!?/, '').slice(0, -1) : ctx.args[0];

        if (!bot.users.get(id)) return await ctx.createMessage(`Ehehe~ I can't find him here ${ctx.author.username}-kun~! Why are you asking me to ignore them?`);
    
        let newBlacklist = bot.blacklist.concat(id);
        let data = {admins: bot.blacklist, blacklist: newBlacklist};

        fs.writeFileSync(`./data/data.json`, JSON.stringify(data));
        bot.blacklist.push(id);
        await ctx.createMessage(`I won't talk to **${utils.formatUsername(bot.users.get(id))}** anymore.`);
    }
};

exports.remove = {
    desc: 'Remove a user from the blacklist.',
    usage: '<mention|ID>',
    async main(bot, ctx) {
        if (!/^(<@!?\d+>|\d+)$/.test(ctx.args[0])) return await ctx.createMessage('Please mention the user to add, or their id.');

        let id = /^<@!?\d+>$/.test(ctx.args[0]) ? ctx.args[0].replace(/^<@!?/, '').slice(0, -1) : ctx.args[0];

        if (!bot.blacklist.includes(id)) return await ctx.createMessage(`Huh, you did not seem to ask me to ignore him, ${ctx.author.username}-kun..` );

        let newBlacklist = bot.blacklist.filter(b => b !== id);
        let data = {admins: bot.admins, blacklist: newBlacklist};

        fs.writeFileSync(`./data/data.json`, JSON.stringify(data));
        bot.blacklist.splice(bot.blacklist.indexOf(id), 1);

        if (!bot.users.get(id)) {
            await ctx.createMessage(`I will no longer ignore **${id}**.`);
        } else {
            await ctx.createMessage(`I will no longer ignore **${utils.formatUsername(bot.users.get(id))}**.`);
        }
    }
};
/**
 * @file Poem Generator command
 * @author Capuccino
 * @author Ovyerus
 */
exports.commands = [
    'poem'
];

exports.poem = {
    desc: 'Make a poem!',
    usage: '<yuri | yuri2 | yuri3 | natsuki | monika | sayori> <poem>',
    main(bot, ctx) {
        let allowedNames = [
            `${[Ss]}ayori`,
            `${[Yy]}uri`,
            `${[Yy]}uri2`,
            `${[Yy]}uri3`,
            `${[Nn]}atsuki`,
            `${[Mm]}onika`,
        ]
        if (!typeof ctx.args[1] === allowedNames) {
            await ctx.createMessage('Hey, Provide me which kind of writing style I should use. run "help poem" for the acceptable names.');
        } else if (!ctx.args[2]) {
            await ctx.createMessage('Give me a poem to write with.');
        } else {
            //TODO: CC @ovyerus for the Python Image generator RPC
            return new Error('Not implemented yet.');
        }
    }
}
/**
 * @file Ecosystem file for PM2.
 * @description: Sysadmins: this is what the service file will execute.
 */
module.exports = {
    apps: [
        {
            name: 'Monika.Gateway',
            script: './Clara/bot.js',
            error_file: '../logs/Gateway/err.log',
            out_file: '../logs/Gateway/out.log',
            log_date_format: 'YYYY-MM-DD HH:mm Z',
            watch: ['./Clara/bot.js'],
            ignore_watch: ['data', 'logs', 'cmd', 'cache', 'node_modules'],
            args: ['--color']
        },
    ]
};
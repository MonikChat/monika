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
        
        {
            name: 'Monika.PoemService',
            script: './Sayori/main.py',
            interpreter: '/usr/bin/python3.6',
            error_file: '../logs/PoemService/err.log',
            out_file: '../logs/PoemService/out.log',
            log_date_format: 'YYYY-MM-DD HH:mm Z',
            watch: ['./Sayori/main.py'],
            ignore_watch: ['data', 'logs', 'cmd', 'cache', 'node_modules'],
            args: ['--color']
        },
        
        {
            name: 'Monika.ContextualChatbot',
            script: './Rebecca/src/chatbot.py',
            interperter: '/usr/bin/python3.6',
            error_file: '../logs/Chatbot/err.log',
            out_file: '../logs/Chatbot/out.log',
            log_date_format: 'YYYY-MM-DD HH:mm Z',
            watch: ['./Rebecca/src/chatbot.py'],
            ignore_watch: ['data', 'logs', 'cmd', 'cache', 'node_modules'],
            args: ['--color']
        },


    ]
};
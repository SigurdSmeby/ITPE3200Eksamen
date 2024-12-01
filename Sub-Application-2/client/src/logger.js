import log from 'loglevel';

// Set the default log level based on the environment
if (process.env.NODE_ENV === 'production') {
    log.setLevel('warn'); // Log only warnings and errors in production
} else {
    log.setLevel('debug'); // Log everything in development
}

// Export the logger
export default log;

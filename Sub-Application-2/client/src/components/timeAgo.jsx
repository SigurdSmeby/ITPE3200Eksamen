/**
 * Calculates the time difference between the current time and a given date string.
 * Returns a human-readable time format like "2h", "3d", etc.
 *
 * @param {string} dateString - The date string to compare.
 * @returns {string} - The time difference in a readable format.
 */
export const timeAgo = (dateString) => {
    const now = new Date();
    const postDate = new Date(dateString);
    const diffInSeconds = Math.floor(
        (now.getTime() - postDate.getTime()) / 1000,
    );

    const minutes = Math.floor(diffInSeconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);
    const weeks = Math.floor(days / 7);

    if (weeks > 0) return `${weeks}w`;
    if (days > 0) return `${days}d`;
    if (hours > 0) return `${hours}h`;
    if (minutes > 0) return `${minutes}m`;
    return `${diffInSeconds}s`;
};

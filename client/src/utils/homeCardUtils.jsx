export const homeCardStatusRenderer = (status, error, loadingMessage, entityName) => {
    if (status === 'loading') {
        return <p>{loadingMessage || `Loading ${entityName}...`}</p>;
    }

    if (status === 'failed') {
        return <p>Error: {error}</p>;
    }

    return null;
};


export const formatDateTime = (dateTime) => {
    return new Date(dateTime).toLocaleDateString('en-US', {
        weekday: 'long', // e.g., "Sunday"
        year: 'numeric', // e.g., "2025"
        month: 'long', // e.g., "May"
        day: 'numeric', // e.g., "4"
        hour: '2-digit', // e.g., "01 PM"
        minute: '2-digit', // e.g., "21"
    });
};
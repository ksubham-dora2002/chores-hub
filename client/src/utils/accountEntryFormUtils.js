
export const trimAndValidateContent = (content, max = 150) => {
    const trimmedContent = content.trim();
    if (trimmedContent.length > max) {
        toast.error("Entry content should not exceed 150 characters.");
        return null;
    }
    return trimmedContent;
}



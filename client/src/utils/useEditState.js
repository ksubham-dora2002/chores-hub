// src/utils/useEditState.js
import { useState, useCallback } from "react";

export function useEditState() {
    const [editId, setEditId] = useState(null);
    const [editContent, setEditContent] = useState("");

    const handleEdit = useCallback((id, oldContent) => {
        setEditId(id);
        setEditContent(oldContent);
    }, []);

    const resetEdit = useCallback(() => {
        setEditId(null);
        setEditContent("");
    }, []);

    return { editId, editContent, setEditId, setEditContent, handleEdit, resetEdit };
}

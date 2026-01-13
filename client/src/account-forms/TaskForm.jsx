import { createTask, fetchTasksByUserEmail, updateTask, deleteTask } from '../_store/task.slice';
import { toast } from 'react-toastify';
import { useDispatch, useSelector } from 'react-redux';
import { toastSuccessWithDelay } from '../utils/toast';
import { trimAndValidateContent } from '../utils/accountEntryFormUtils';
import { useState, useEffect } from 'react';

import { useEditState } from "../utils/useEditState";




export const TaskForm = () => {

  const dispatch = useDispatch();
  const currentUserId = useSelector((state) => state.auth.id);
  const { taskListByEmail, pageSize } = useSelector((state) => state.task);

  const [content, setContent] = useState("");
  const { editId, editContent, 
    setEditContent, setEditId, 
    handleEdit, resetEdit } = useEditState();

  useEffect(() => {
    if (currentUserId) {
      dispatch(fetchTasksByUserEmail({ pageSize }));
    }
  }, [dispatch, pageSize]);

  // CREATE HANDLER
  const handleCreate = async (e) => {
    e.preventDefault();
    const currentDate = new Date().toISOString();
    const trimmedContent = trimAndValidateContent(content, 150);

    const result = await dispatch(createTask({ content: trimmedContent, 
      date: currentDate, 
      isDone: false, 
      userId: currentUserId }));

    if (createTask.fulfilled.match(result)) {
      toastSuccessWithDelay(50, "Task entry created!");
      setContent("");
      dispatch(fetchTasksByUserEmail({ pageSize }));
    } else if (createTask.rejected.match(result)) {
      toast.error(result.payload || "Failed to create task entry");
    }
  };

  // UPDATE HANDLER
  const handleUpdate = async (e, id) => {
    e.preventDefault();
    const currentDate = new Date().toISOString();
    const trimmedContent = trimAndValidateContent(editContent, 150);

    const result = await dispatch(updateTask({ id, 
      content: trimmedContent, 
      date: currentDate, 
      userId: currentUserId, 
      isDone: false }));

    if (updateTask.fulfilled.match(result)) {
      toastSuccessWithDelay(50, "Task entry updated!");
      resetEdit();
      dispatch(fetchTasksByUserEmail({ pageSize }));
    } else if (updateTask.rejected.match(result)) {
      toast.error(result.payload || "Failed to update task");
    }
  };

  // DELETE HANDLER
  const handleDelete = async (e, id) => {
    e.preventDefault();
    const result = await dispatch(deleteTask(id));

    if (deleteTask.fulfilled.match(result)) {
      toastSuccessWithDelay(50, "Task entry deleted!");
      dispatch(fetchTasksByUserEmail({ pageSize }));
    } else if (deleteTask.rejected.match(result)) {
      toast.error(result.payload || "Failed to delete task entry");
    }
  };


  return (
    <>
      <div className="entry">
        <div className="entry__create-card">
          <h3 className="entry__title">Task</h3>
          <form className="entry__form" onSubmit={handleCreate}>
            <div className="entry__form-box--ver">
              <label className="entry__form-label" htmlFor="taskEntityTxt">Enter Task</label>
              <textarea
                name="taskEntityTxt"
                id="taskEntityTxt"
                className="entry__textarea"
                value={content}
                onChange={(e) => setContent(e.target.value.replace(/[\r\n]+/g, ' '))}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') e.preventDefault();
                }}
                required
              ></textarea>
            </div>
            <button className="entry__form-btn btn-stand" type="submit">Create</button>
          </form>
        </div>
        <div className="entry__list-card">
          <h3 className="entry__list-title">Task Entries</h3>
          <ul className="entry__list">
            {taskListByEmail && taskListByEmail.length > 0 ? (
              taskListByEmail.map((task) => (
                <li className="entry__item" key={task.id}>
                  <div className="entry__item-body">
                    <div className="entry__item-status">
                      {task.isDone ? (
                        <>
                          <span className="material-symbols-rounded entry__item-icon">
                            check_circle
                          </span>
                          <span className="entry__item-status-text">Done</span>
                        </>
                      ) : (<>
                        <span className="material-symbols-rounded entry__item-icon">
                          cancel
                        </span>
                        <span className='entry__item-status-text'>Not Done</span>
                      </>
                      )}
                    </div>
                    <div className="entry__item-content">
                      <p className="entry__item-text">{task.content}</p>
                      <span className="material-symbols-rounded entry__item-edit-icon" onClick={() => handleEdit(task.id, task.content)}>
                        edit
                      </span>
                    </div>
                  </div>
                  {editId === task.id ? (
                    <form className=" entry__item-form" onSubmit={(e) => handleUpdate(e, task.id)}>
                      <input
                        className="entry__item-input"
                        type="text"
                        name='updateTaskInp'
                        value={editContent}
                        onChange={(e) => setEditContent(e.target.value)}
                        required
                      />
                      <button className="entry__item-btn--update entry__item-btn btn-stand" type="submit">Update</button>
                      <button
                        className="entry__item-btn btn-stand"
                        type="button"
                        onClick={() => setEditId(null)}
                      >Cancel</button>
                    </form>
                  ) : (
                    <form className="entry__item-form--delete entry__item-form" onSubmit={(e) => handleDelete(e, task.id)}>
                      <button
                        className="entry__item-btn entry__item-btn--delete btn-stand"
                        type="submit"
                      >Delete</button>
                    </form>
                  )}
                </li>
              ))
            ) : (
                <li className='entry__item-warning'>No task entries found.</li>
            )}
          </ul>
        </div>
      </div>
    </>
  )
}


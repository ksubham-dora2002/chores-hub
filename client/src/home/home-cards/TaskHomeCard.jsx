import { fetchTasks, updateTask } from '../../_store/task.slice';
import { useSelector, useDispatch } from 'react-redux';
import { toast } from 'react-toastify';
import { homeCardStatusRenderer, formatDateTime } from '../../utils/homeCardUtils';
const def_avatar = "/def_avatar.svg";
import { useEffect } from 'react'

export const TaskHomeCard = () => {
    const dispatch = useDispatch();
    const { taskList, status, pageSize, error } = useSelector((state) => state.task);

    useEffect(() => {
        if (status === 'idle') {
            dispatch(fetchTasks({ pageSize }));
        }
    }, [dispatch, status, pageSize, taskList.length]);

    homeCardStatusRenderer(status, error, 'Loading tasks...', 'tasks');

    // UPDATE HANDLER 
    const handleUpdatingTaskDone = async (task) => {
        const response = await dispatch(updateTask({
            ...task,
            isDone: true,
            userPicture: task.userPicture || def_avatar,
        }));
        if (updateTask.fulfilled.match(response)) {
            await dispatch(fetchTasks({ pageSize }));
            toast.success("Task marked as done.");
        } else if (updateTask.rejected.match(response)) {
            toast.error(response.payload || "Failed to mark task as done");
        }
    };


    return (
        <>{taskList.length > 0 ? (
            taskList.map((task) => (
                <div key={task.id} className="home-card home-card--task">
                    <div className="home-card__header">
                        <h4 className="home-card__title home-card__title--task">Task</h4>
                    </div>
                    <div className="home-card__body">
                        <div className=" home-card__user">
                            <img className="home-card__avatar" src={task.userPicture || def_avatar} alt={task.userName || 'User'} />
                            <span className="home-card__user-name">{task.userName || 'Unknown User'}</span>
                        </div>
                        <div className="home-card__content">
                            <p className="home-card__content-text">{task.content}</p>
                            <time className="home-card__date" dateTime={task.createdAt}>
                                {formatDateTime(task.createdAt)}
                            </time>
                        </div>
                        <button className="btn-stand home-card__action-btn"
                            onClick={() => handleUpdatingTaskDone(task)}
                        >Done</button>
                    </div>
                </div>
            ))
        ) : (<p>No tasks available.</p>)}
        </>
    )
}

import { fetchUserById, updateUser } from "../../_store/user.slice";
import { useSelector, useDispatch } from "react-redux";
import { toast } from 'react-toastify';
const def_avatar = "/def_avatar.svg";
import { useState } from "react";
import './AccountForm.css';


export const AccountForm = () => {

  const dispatch = useDispatch();
  const user = useSelector((state) => state.user);
  const [selectedFile, setSelectedFile] = useState(null);
  const currentId = useSelector((state) => state.auth.id)


  const handleFileChange = (event) => {
    const file = event.target.files[0];
    if (file) {
      setSelectedFile(file);
    }
  }

  // UPDATE USER HANDLER
  const handleUpdate = async (e) => {
    e.preventDefault();

    const loadingToast = toast.info("Updating user. Please wait...", { autoClose: false });

    const formData = {
      id: currentId,
      email: e.target.email.value,
      fullName: e.target.fullName.value,
      profilePicture: selectedFile,
    };

    const result = await dispatch(updateUser(formData));

    toast.dismiss(loadingToast);

    if (updateUser.fulfilled.match(result)) {
      toast.success("User updated successfully!");
      setTimeout(() => {
        dispatch(fetchUserById(currentId));
      }, 250);
    } else if (updateUser.rejected.match(result)) {
      toast.error(result.payload || "Failed to update user. Please try again.");
    }
    setSelectedFile(null);
    e.target.reset();
  };

  return (
    <>
      <div className="account-card account-card--profile">
        <form className="account-card__form" onSubmit={handleUpdate}>
          <div className="account-card__avatar">
            <img src={
              selectedFile
                ? URL.createObjectURL(selectedFile)
                : user.profilePicture || def_avatar
            } alt="" className="account-card__avatar-img" />
            <label htmlFor="updateAvatar" className="account-card__avatar-label">Change Profile Photo</label>
            <input
              className='account-card__avatar-input'
              type="file"
              id="updateAvatar"
              accept='image/*'
              style={{ display: 'none' }}
              onChange={handleFileChange} />
            <button
              type="button"
              className="account-card__avatar-button btn-stand"
              onClick={() => document.getElementById('updateAvatar').click()}
            >
              Choose File
            </button>
          </div>
          <div className="account-card__form-box form-box">
            <label className='account-card__form-label' htmlFor="fullName">Full name</label>
            <input
              id='fullName'
              className='account-card__form-input form-input' type="text"
              placeholder='full name'
              defaultValue={user.fullName} />
          </div>
          <div className="account-card__form-box form-box">
            <label className='account-card__form-label' htmlFor="email">Email</label>
            <input
              id='email'
              className='account-card__form-input form-input' type="email"
              placeholder='email'
              defaultValue={user.email}
            />
          </div>
          <button className='account-card__form-btn btn-stand' type="submit">Update</button>
        </form>
      </div>
    </>
  )
}

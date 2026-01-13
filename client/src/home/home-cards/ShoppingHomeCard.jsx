import { fetchShopProducts, updateShopProduct } from '../../_store/shopping.slice';
import { useSelector, useDispatch } from 'react-redux';
import { toast } from 'react-toastify';
import { homeCardStatusRenderer, formatDateTime } from '../../utils/homeCardUtils';
const def_avatar = "/def_avatar.svg";
import { useEffect } from 'react';


export const ShoppingHomeCard = () => {

  const dispatch = useDispatch();
  const { shopProductList, status, pageSize, error } = useSelector((state) => state.shopping);

  useEffect(() => {
    if (status === 'idle') {
      dispatch(fetchShopProducts({ pageSize }));
    }
  }, [dispatch, pageSize]);

  homeCardStatusRenderer(status, error, 'Loading shopping products...', 'shopping products');

  // UPDATE HANDLER
  const handleUpdatingShopProductBought = async (product) => {
    const response = await dispatch(updateShopProduct({
      ...product,
      isBought: true,
      userPicture: product.userPicture || def_avatar,
    }));
    if (updateShopProduct.fulfilled.match(response)) {
      await dispatch(fetchShopProducts({ pageSize }));
      toast.success("Product marked as bought.");
    } else if (updateShopProduct.rejected.match(response)) {
      toast.error(response.payload || "Failed to mark product as bought");
    }
  };


  return (
    <>
      {shopProductList.length > 0 ? (
        shopProductList.map((product) => (
          <div key={product.id} className="home-card home-card--shopping">
            <div className="home-card__header">
              <h4 className="home-card__title">Shopping</h4>
            </div>
            <div className="home-card__body">
              <div className="home-card__user">
                <img className="home-card__avatar" src={product.userPicture || def_avatar} alt={product.userName || "User"} />
                <span className="home-card__user-name">{product.userName}</span>
              </div>
              <div className="home-card__content">
                <p className="home-card__content-text">{product.content}</p>
                <time className="home-card__date" dateTime={product.createdAt}>
                  {formatDateTime(product.createdAt)}
                </time>
              </div>
              <button className="btn-stand home-card__action-btn"
                onClick={() => handleUpdatingShopProductBought(product)}
              >Bought</button>
            </div>
          </div>
        ))) : (<p>No products available.</p>)}
    </>
  )
}

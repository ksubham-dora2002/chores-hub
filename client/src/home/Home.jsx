import { NotificationHomeCard, TaskHomeCard, ShoppingHomeCard } from "./home-cards/index";
import "./Home.css";


export const Home = () => {

  return <>
    <section className="home">
      <div className="home__cards">
        <NotificationHomeCard />
        <TaskHomeCard />
        <ShoppingHomeCard />
      </div>
    </section>
  </>;

};

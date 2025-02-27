import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import { getUrlStats } from "../api";
import Template from "../components/Template";

const StatsPage = () => {
  const location = useLocation();
  const params = new URLSearchParams(location.search);
  const shortUrl = params.get("url");

  const [clicks, setClicks] = useState<number | null>(null);

  useEffect(() => {
    if (shortUrl) {
      getUrlStats(shortUrl)
        .then((data) => {
          setClicks(data.clicks);
        })
        .catch((error) => console.error("Ошибка при получении статистики", error));
    }
  }, [shortUrl]);

  return (
    <Template>
      <div className="stats-box">
        <h2>Статистика по ссылке</h2>
        {shortUrl && (
          <p>
            Ссылка:{" "}
            <a href={shortUrl} target="_blank" rel="noopener noreferrer">
              {shortUrl}
            </a>
          </p>
        )}
        {clicks !== null && clicks !== undefined ? (
          <p>Количество переходов: {clicks}</p>
        ) : (
          <p>Нет данных о кликах</p>
        )}
      </div>
    </Template>
  );
};

export default StatsPage;

import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getUrlStats } from "../api";
import Template from "../components/Template";

const StatsPage = () => {
  const { shortCode } = useParams<{ shortCode: string }>();

  const [clicks, setClicks] = useState<number | null>(null);
  const [shortUrl, setShortUrl] = useState<string | null>(null);

  useEffect(() => {
    if (shortCode) {
      getUrlStats(shortCode)
        .then((data) => {
          setShortUrl(data.shortUrl);
          setClicks(data.clicks);
        })
        .catch((error) => console.error("Ошибка при получении статистики", error));
    }
  }, [shortCode]);

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
        {clicks !== null ? (
          <p>Количество переходов: {clicks}</p>
        ) : (
          <p>Нет данных о кликах</p>
        )}
      </div>
    </Template>
  );
};

export default StatsPage;

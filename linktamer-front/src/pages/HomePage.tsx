import { useState } from "react";
import Template from "../components/Template";
import { shortenUrl } from "../api";
import { useNavigate } from "react-router-dom";
import API_BASE_URL from "../config";

const HomePage = () => {
  const [originalUrl, setOriginalUrl] = useState<string>("");
  const [shortUrl, setShortUrl] = useState<string>("");
  const [stats, setStats] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleShorten = async () => {
    if (!originalUrl.trim()) {
      setError("URL не может быть пустым.");
      return;
    }

    try {
      setError(null);
      const data = await shortenUrl(originalUrl);
      setShortUrl(data.shortUrl);
    } catch (error) {
      console.error("Ошибка при сокращении ссылки", error);
      setError("Произошла ошибка при сокращении ссылки.");
    }
  };

  const navigate = useNavigate();

  const handleGetStats = () => {
    if (!shortUrl) return;
    navigate(`/stats?url=${encodeURIComponent(shortUrl)}`);
  };

  return (
    <Template>
      <div className="shortener-box">
        <input
          type="text"
          className={`url-input ${error ? "error" : ""}`}
          placeholder="Вставьте ссылку"
          value={originalUrl}
          onChange={(e) => setOriginalUrl(e.target.value)}
        />
        <button className="shorten-button" onClick={handleShorten}>
          Сократить
        </button>
        {error && <p className="error-message">{error}</p>}
        {shortUrl && (
          <>
            <input
              type="text"
              className="url-input"
              value={shortUrl}
              readOnly
            />
            <div className="button-container">
              <button className="stats-button" onClick={handleGetStats}>
                Показать статистику
              </button>
              <button
                className="redirect-button"
                onClick={() => window.open(`${API_BASE_URL}/redirect/${encodeURIComponent(shortUrl)}`, "_blank", "noopener,noreferrer")}
              >
                Перейти по ссылке
              </button>
            </div>
            {stats !== null && <p className="stats-text">Количество переходов: {stats}</p>}
          </>
        )}
      </div>
    </Template>
  );
};

export default HomePage;

import React, { ReactNode } from "react";
import "../styles.css";

type TemplateProps = {
  children: ReactNode;
};

const Template = ({ children }: TemplateProps) => {
  return (
    <div className="container">
      <header className="header">Укротитель ссылок</header>
      <main className="content">{children}</main>
      <footer className="footer">© 2025 LinkTamer</footer>
    </div>
  );
};

export default Template;

import './App.css'

function App() {
  return (
    <div className="app">
      <header className="hero">
        <div className="badge">ProudSmileDent</div>
        <h1>Healthy Smile, Better Life</h1>
        <p>
          ProudSmileDent — pasiyentlər üçün stomatoloji xidmətləri göstərən
          sadə React frontend tətbiqidir.
        </p>

        <div className="buttons">
          <a href="#services">Xidmətlər</a>
          <a href="#about" className="secondary">Haqqımızda</a>
        </div>
      </header>

      <section id="services" className="cards">
        <div className="card">
          <h2>Diş müayinəsi</h2>
          <p>Pasiyentlər klinikanın əsas xidmətləri ilə tanış ola bilər.</p>
        </div>

        <div className="card">
          <h2>Rezervasiya</h2>
          <p>Gələcəkdə istifadəçi görüş vaxtı seçərək rezervasiya edə bilər.</p>
        </div>

        <div className="card">
          <h2>Admin panel</h2>
          <p>Klinika işçiləri xidmətləri və müraciətləri idarə edə bilər.</p>
        </div>
      </section>

      <section id="about" className="about">
    
      </section>
    </div>
  )
}

export default App
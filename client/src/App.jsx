import logo from './logo.svg';
import './App.css';
import NavMenu from './components/shared/navbar.tsx';


function App() {
  return (
    <div className="App">
      <NavMenu />
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
        <MyApp name="kanin" kjønn="dame"/>
      </header>
    </div>
  );
}
function MyButton(kaniner) {
  return (
    
    <button>I'm a {kaniner.name}</button>
  );
}
function MyApp(kaniner) {
  return (
    <div>
      <h1>Welcome to my app, min {kaniner.kjønn}</h1>
      <MyButton name={kaniner.name}/>
    </div>
  );
}

export default App;

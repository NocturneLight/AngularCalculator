import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { publish } from 'rxjs';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

interface Calculator {
  //value: number;
  //formattedValue: string;
  equation: string;
  calculation: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  preserveWhitespaces: true
})

export class AppComponent implements OnInit {
  public forecasts: WeatherForecast[] = [];
  public CurrentEquation = "";
  public CurrentCalculation = "";

  // Each button we will have for the calculator.
  data = [
    "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "+", "-", "*", "/", "%"
  ];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getForecasts();
  }

  getForecasts() {
    this.http.get<WeatherForecast[]>('api/weatherforecast').subscribe(
      (result) => {
        this.forecasts = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  // Sends a given value associated with a button to the server and then
  // returns a formatted string.
  public GetNumber(value: string) {
    const char = encodeURIComponent(value);

    this.http.post<Calculator>(`api/number?number=${char}`, {Sender: char}).subscribe({
      next: (result) => {
        this.CurrentEquation = this.CurrentEquation + result.equation;
      },

      error: (error) => { console.error(error) }
    });
  }

  // Sends an equation to the server and returns the calculated result.
  public Calculate() {
    const char = encodeURIComponent(this.CurrentEquation);

    this.http.get<Calculator>(`api/number?equation=${char}`).subscribe({
      next: (result) => { this.CurrentCalculation = result.calculation },

      error: (error) => { console.error(error) }
    });
  }

  title = 'angularcalculator.client';
}

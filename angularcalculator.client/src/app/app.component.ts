import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

interface Calculator {
  Value: number;
  FormattedValue: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  preserveWhitespaces: true
})

export class AppComponent implements OnInit {
  public forecasts: WeatherForecast[] = [];
  public Calculator: Calculator = { Value: NaN, FormattedValue: "" };
  public CurrentEquation = "";

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

  public getNumber(value: string) {
    const char = encodeURIComponent(value);

    this.http.post<Calculator>(`api/number?number=${char}`, {Sender: char}).subscribe({
      next: (result) => {
        //this.Calculator = result
        this.CurrentEquation = this.CurrentEquation + result.FormattedValue;
      },
      error: (error) => { console.error(error) }
    });
  }

  title = 'angularcalculator.client';
}

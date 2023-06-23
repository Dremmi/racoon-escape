using UnityEngine;
using UniRx;

public readonly struct OnRaceSalaryCountMessage
{
    public readonly int Salary;
    public readonly bool HasFinished;

    public OnRaceSalaryCountMessage(int salary, bool hasFinished)
    {
        Salary = salary;
        HasFinished = hasFinished;
    }
}
public class PlayerMoney : MonoBehaviour
{
    private PlayerActiveCar _activeCar;

    private PlayerMoneyConfig _moneyConfig;

    private int _moneyPer100Meters;

    private int _earnedMoneyInCurrentRide;
    private int _earnedMoneyForDistance;    

    private bool _isEarningMoney;

    public void SetActiveCar(PlayerActiveCar car)
    {
        _activeCar = car;
    }

    public int UpdateCurrentRaceEarning()
    {
        return _earnedMoneyInCurrentRide;
    }

    private void Awake()
    {
        _moneyConfig = GetComponent<ApplicationStartUp>().PlayerMoneyConfig;
        _moneyPer100Meters = _moneyConfig.MoneyDistance100Meters;

        MessageBroker
            .Default
            .Receive<OnGameStartMessage>()
            .Subscribe(message =>
            {
                _earnedMoneyInCurrentRide = 0;
                _earnedMoneyForDistance = 0;
                _isEarningMoney = true;                
            });

        MessageBroker
            .Default
            .Receive<OnPlayerDefeatedMessage>()
            .Subscribe(message =>
            {                
                MessageBroker
                .Default
                .Publish(new OnRaceSalaryCountMessage(_earnedMoneyInCurrentRide, true));
                _isEarningMoney = false;
            });
    }

    private void FixedUpdate()
    {
        if (_isEarningMoney)
        {
            EarnMoney();
        }
    }

    private void EarnMoney()
    {
        EarnMoneyForDistance();
        _earnedMoneyInCurrentRide = _earnedMoneyForDistance;
    }

    private void EarnMoneyForDistance()
    {
        _earnedMoneyForDistance = _moneyPer100Meters * (_activeCar.GetCurrentRideDistance() / 100);
    }

}

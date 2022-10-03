using FluentAssertions;
using MockingUnitTestsDemoApp.Impl.Models;
using MockingUnitTestsDemoApp.Impl.Repositories.Interfaces;
using MockingUnitTestsDemoApp.Impl.Services;
using Moq;

namespace MockingUnitTestsDemoApp.Tests
{
    public class PlayerServiceTests
    {
        private readonly Mock<IPlayerRepository> _playerRepoMock;
        private readonly Mock<ITeamRepository> _teamRepoMock;
        private readonly Mock<ILeagueRepository> _leagueRepoMock;

        private readonly PlayerService _subject;

        public List<Player> GetFakePlayers(int numberOfTeams)
        {
            var players = new List<Player>();

            for (int t = 0; t < numberOfTeams; t++)
            {
                for (int p = 0; p < 11; p++)
                {
                    var player = new Player()
                    {
                        ID = players.Count,
                        FirstName = "Player",
                        LastName = p.ToString(),
                        DateOfBirth = DateTime.Now,
                        TeamID = t
                    };

                    players.Add(player);
                }
            }           

            return players;
        }

        public List<Team> GetFakeTeams(int leagueID, int numberOfTeams)
        {
            var teams = new List<Team>();

            for (int i = 0; i < numberOfTeams; i++)
            {
                var team = new Team()
                {
                    ID = i,
                    Name = $"Team {i}",
                    LeagueID = leagueID,
                    FoundingDate = DateTime.Now
                };

                teams.Add(team);
            }
            

            return teams;
        }

        public PlayerServiceTests()
        {
            _playerRepoMock = new Mock<IPlayerRepository>();
            _teamRepoMock = new Mock<ITeamRepository>();
            _leagueRepoMock = new Mock<ILeagueRepository>();

            _subject = new PlayerService(_playerRepoMock.Object, _teamRepoMock.Object, _leagueRepoMock.Object);

        }

        [Fact]
        public void GetForLeague_ValidLeague_ShouldWork()
        {
            //Arrange
            var leagueID = 1;
            var numberOfTeams = 3;            
            List<Team> teams = GetFakeTeams(leagueID, numberOfTeams);
            List<Player> players = GetFakePlayers(numberOfTeams);

            _leagueRepoMock
                .Setup(x => x.IsValid(leagueID))
                .Returns(true);

            _teamRepoMock
                .Setup(x => x.GetForLeague(leagueID))
                .Returns(teams);

            for (int i = 0; i < numberOfTeams; i++)
            {
                _playerRepoMock
                .Setup(x => x.GetForTeam(i))
                .Returns(players.Where(x => x.TeamID == i).ToList());
            }

            //Act
            var result = _subject.GetForLeague(leagueID);


            //Assert
            result.Should().ContainInOrder(players);
        }

        [Fact]
        public void GetForLeague_InvalidLeague_ShouldFail()
        {
            //Arrange
            var leagueID = 1;
            var numberOfTeams = 3;            
            List<Team> teams = GetFakeTeams(leagueID, numberOfTeams);
            List<Player> players = GetFakePlayers(numberOfTeams);

            _leagueRepoMock
                .Setup(x => x.IsValid(leagueID))
                .Returns(true);

            _teamRepoMock
                .Setup(x => x.GetForLeague(leagueID))
                .Returns(teams);

            for (int i = 0; i < numberOfTeams; i++)
            {
                _playerRepoMock 
                .Setup(x => x.GetForTeam(i))
                .Returns(players.Where(x => x.TeamID == i).ToList());
            }

            //Act
            var result = _subject.GetForLeague(777);


            //Assert
            result.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GetForLeague_EmptyLeague_ShouldReturnEmpty()
        {
            //Arrange
            var leagueID = 1;
            var numberOfTeams = 3;            
            List<Team> teams = new List<Team>(); //League exists but has zero teams
            List<Player> players = GetFakePlayers(numberOfTeams);

            _leagueRepoMock
                .Setup(x => x.IsValid(leagueID))
                .Returns(true);

            _teamRepoMock
                .Setup(x => x.GetForLeague(leagueID))
                .Returns(teams);

            for (int i = 0; i < numberOfTeams; i++)
            {
                _playerRepoMock
                .Setup(x => x.GetForTeam(i))
                .Returns(players.Where(x => x.TeamID == i).ToList());
            }

            //Act
            var result = _subject.GetForLeague(leagueID);


            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetForLeague_EmptyTeams_ShouldReturnEmpty()
        {
            //Arrange
            var leagueID = 1;
            var numberOfTeams = 3;
            List<Team> teams = GetFakeTeams(leagueID, numberOfTeams);
            List<Player> players = new List<Player>(); //League exists but the teams have zero players

            _leagueRepoMock
                .Setup(x => x.IsValid(leagueID))
                .Returns(true);

            _teamRepoMock
                .Setup(x => x.GetForLeague(leagueID))
                .Returns(teams);

            for (int i = 0; i < numberOfTeams; i++)
            {
                _playerRepoMock
                .Setup(x => x.GetForTeam(i))
                .Returns(players.Where(x => x.TeamID == i).ToList());
            }

            //Act
            var result = _subject.GetForLeague(leagueID);


            //Assert
            result.Should().BeEmpty();
        }
    }
}